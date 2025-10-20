import React from 'react';
import { Modal, Table, Badge } from 'react-bootstrap';

export interface ModelCard {
  id: number;
  usageEntryId: number;
  approvedBy: string;
  approvedAt: string;
  finalRiskLevel: string;
  aiRiskLevel: string;
  aiConfidence?: number;
  aiRationale?: string;
  complianceChecklist: string;
  statusDecision: string;
  policyAlerts?: string;
  notes?: string;
}

interface Props {
  show: boolean;
  onHide: () => void;
  card: ModelCard | null;
}

const RiskBadge: React.FC<{ value: string }> = ({ value }) => {
  const normalized = value.toLowerCase();
  const variant = normalized === 'high' ? 'danger' : normalized === 'medium' ? 'warning' : 'success';
  return <Badge bg={variant} className="text-uppercase">{value}</Badge>;
};

const ModelCardModal: React.FC<Props> = ({ show, onHide, card }) => {
  if (!card) return null;

  const complianceItems = card.complianceChecklist
    ? card.complianceChecklist.split(';').map(item => item.trim()).filter(Boolean)
    : [];

  return (
    <Modal show={show} onHide={onHide} size="lg">
      <Modal.Header closeButton>
        <Modal.Title>Model Card #{card.id}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Table bordered responsive>
          <tbody>
            <tr>
              <th>Approved By</th>
              <td>{card.approvedBy}</td>
            </tr>
            <tr>
              <th>Approved At</th>
              <td>{new Date(card.approvedAt).toLocaleString()}</td>
            </tr>
            <tr>
              <th>Final Decision</th>
              <td>
                <RiskBadge value={card.finalRiskLevel} />
                <span className="ms-2">{card.statusDecision}</span>
              </td>
            </tr>
            <tr>
              <th>AI Assessment</th>
              <td>
                <RiskBadge value={card.aiRiskLevel} />
                {typeof card.aiConfidence === 'number' && (
                  <span className="ms-2">Confidence {(card.aiConfidence * 100).toFixed(1)}%</span>
                )}
                {card.aiRationale && <p className="mb-0 mt-2">{card.aiRationale}</p>}
              </td>
            </tr>
            <tr>
              <th>Policy Alerts</th>
              <td>{card.policyAlerts || 'None'}</td>
            </tr>
            <tr>
              <th>Compliance Checklist</th>
              <td>
                {complianceItems.length > 0 ? (
                  <ul className="mb-0">
                    {complianceItems.map(item => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                ) : (
                  'None'
                )}
              </td>
            </tr>
            <tr>
              <th>Notes</th>
              <td>{card.notes || '—'}</td>
            </tr>
          </tbody>
        </Table>
      </Modal.Body>
    </Modal>
  );
};

export default ModelCardModal;
