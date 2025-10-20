# 🧠 AI Governance Portal

> **Track, manage, and govern AI tool usage in your organization.**

---

## 🚀 Overview

The **AI Governance Portal** is a full-stack web application designed to help organizations monitor and control the use of AI tools by employees. With secure authentication, role-based access, and a modern UI, this project demonstrates best practices in both frontend and backend development.

---

## 🌟 Features

- **User Authentication & Authorization**
  Secure registration and login with JWT-based authentication. Supports employee and admin roles.

- **Role-Based Access Control**
  Admins can view and manage all AI usage entries; employees can only see and edit their own.

- **AI Usage Tracking**
  Employees can log their use of AI tools, including purpose, data type, and risk level.

- **ML-powered Risk Scoring**
  A lightweight Hugging Face classifier (tiny DistilBERT) generates advisory risk ratings with explainability and confidence scores.

- **Adversarial Prompt Shielding**
  Incoming submissions are scanned for prompt-injection patterns and flagged for manual escalation.

- **Automated Regulatory Mapping**
  The AI-derived risk profile is tied to GDPR/HIPAA/PCI checklists and auto-denies severe violations.

- **Model Cards & Audit Trail**
  Every approved request snapshots its decision context into a model card for downstream governance reviews.

- **Responsive UI**
  Built with React and Bootstrap for a seamless experience on any device.

- **RESTful API**
  .NET Minimal API backend with full CRUD support and protected endpoints.

- **Persistent Data**  
  All data is stored in a SQLite database via Entity Framework Core.

---

## 🛠️ Tech Stack

| Frontend         | Backend         | Database | Auth         | Styling   |
|------------------|----------------|----------|--------------|-----------|
| React (Vite)     | .NET 8 Minimal API | SQLite   | JWT (OAuth2.0) | Bootstrap |

---

## 📸 Screenshots

![Login Page](./screenshots/login.png)
![Dashboard](./screenshots/dashboard.png)
![Usage Form](./screenshots/usage-form.png)

---

## 🏁 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js & npm](https://nodejs.org/)
- [Python 3.11](https://www.python.org/) (for the local ML risk microservice)
- [pip](https://pip.pypa.io/) and [virtualenv](https://virtualenv.pypa.io/) recommended
- [Vite](https://vitejs.dev/)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ai-governance-portal.git
   cd ai-governance-portal
   ```

2. **Risk Scoring Microservice**
   ```bash
   cd ml-service
   python -m venv .venv
   source .venv/bin/activate  # Windows: .venv\Scripts\activate
   pip install -r requirements.txt
   uvicorn app:app --reload --port 8000
   ```

3. **Backend**
   ```bash
   cd backend
   dotnet ef database update   # Apply migrations
   dotnet run
   ```

4. **Frontend**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

5. **Visit**
   Open [http://localhost:5173](http://localhost:5173) in your browser.

> ℹ️ The backend expects the ML service at `http://localhost:8000`; override with `RiskAssessment__BaseUrl` if needed.

---

## 🐳 Run Everything with Docker Compose

1. Build and start all services (frontend, backend, ML) in one step:
   ```bash
   docker compose up --build
   ```
2. Visit the frontend on [http://localhost:5173](http://localhost:5173). The backend listens on port `5056`, and the ML API on `8000`.
3. Stop the stack with `Ctrl+C` and `docker compose down` when finished.

---

## 🔒 Authentication

- **Register** as a new user (employee by default).
- **Login** to access the portal.
- **Admins** can manage all entries; employees see only their own.

---

## 📂 Project Structure

```
/backend   # .NET Minimal API, Entity Framework, SQLite
/frontend  # React, Vite, Bootstrap, JWT Auth
```

---

## 💡 Why This Project?

AI is transforming workplaces, but governance is critical. This portal provides a practical solution for organizations to ensure responsible AI usage, while showcasing my skills in secure, scalable, and modern web development.

---

## 🚀 Future Improvements

- Email verification & password reset
- Audit logs & advanced analytics
- Cloud deployment (Azure/AWS)
- Automated testing (unit & integration)

---

## 👋 About Me

I'm passionate about building impactful software with modern technologies.  
**Let’s connect!**  
[LinkedIn](https://www.linkedin.com/in/mansurisaad555)

---

## 📜 License

MIT

---

> _Built with ❤️ by Saad Mansuri