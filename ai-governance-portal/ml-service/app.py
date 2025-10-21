from __future__ import annotations

from typing import List

from fastapi import FastAPI
from pydantic import BaseModel
import os
from transformers import pipeline
from typing import Optional

try:
    from huggingface_hub import snapshot_download
except Exception:
    snapshot_download = None  # Optional fallback if hub not available

MODEL_ID = os.environ.get(
    "MODEL_ID",
    "distilbert-base-uncased-finetuned-sst-2-english",
)

def _load_pipeline(model_id: str):
    try:
        return pipeline(
            "text-classification",
            model=model_id,
            framework="pt",
            device=-1,
        )
    except Exception:
        cache_dir = os.environ.get("HF_HOME") or os.environ.get("TRANSFORMERS_CACHE") or "/app/.cache"
        if snapshot_download is not None:
            try:
                snapshot_download(
                    repo_id=model_id,
                    cache_dir=cache_dir,
                    force_download=True,
                    local_files_only=False,
                    resume_download=False,
                )
            except Exception:
                pass
        # Retry once after forcing a fresh download
        try:
            return pipeline(
                "text-classification",
                model=model_id,
                framework="pt",
                device=-1,
            )
        except Exception:
            fallback_id = "distilbert-base-uncased-finetuned-sst-2-english"
            if model_id != fallback_id:
                return pipeline(
                    "text-classification",
                    model=fallback_id,
                    framework="pt",
                    device=-1,
                )
            raise


txt_classifier = _load_pipeline(MODEL_ID)

RISK_THRESHOLDS = {
    "low": 0.80,
    "medium": 0.55
}

COMPLIANCE_KEYWORDS = {
    "gdpr": ["gdpr", "general data protection regulation", "eu personal data"],
    "hipaa": ["hipaa", "phi", "patient", "medical"],
    "financial": ["pci", "credit card", "ssn", "social security", "financial"],
}


class AssessmentRequest(BaseModel):
    toolName: str
    dataType: str
    purpose: str


class AssessmentResponse(BaseModel):
    riskLevel: str
    confidence: float
    rationale: str
    modelName: str
    modelVersion: str
    policyAlerts: List[str]


app = FastAPI(title="AI Governance Risk Service")


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok", "model": MODEL_ID}


@app.post("/assess", response_model=AssessmentResponse)
def assess(payload: AssessmentRequest) -> AssessmentResponse:
    combined = f"Tool: {payload.toolName}\nData Type: {payload.dataType}\nPurpose: {payload.purpose}"
    result = txt_classifier(combined)[0]

    label = result["label"].upper()
    score = float(result["score"])

    if label == "NEGATIVE":
        risk_level = "High"
    elif score >= RISK_THRESHOLDS["low"]:
        risk_level = "Low"
    elif score >= RISK_THRESHOLDS["medium"]:
        risk_level = "Medium"
    else:
        risk_level = "High"

    alerts: list[str] = []
    searchable = f"{payload.toolName} {payload.dataType} {payload.purpose}".lower()
    for tag, words in COMPLIANCE_KEYWORDS.items():
        if any(word in searchable for word in words):
            alerts.append(tag.upper())

    rationale = (
        f"Model predicted {label} sentiment with confidence {score:.2f}. "
        f"Mapped to {risk_level} risk using lightweight heuristics."
    )

    return AssessmentResponse(
        riskLevel=risk_level,
        confidence=score,
        rationale=rationale,
        modelName=MODEL_ID,
        modelVersion=getattr(txt_classifier.model.config, "_name_or_path", MODEL_ID),
        policyAlerts=alerts,
    )
