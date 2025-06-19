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
- [Vite](https://vitejs.dev/)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ai-governance-portal.git
   cd ai-governance-portal
   ```

2. **Backend**
   ```bash
   cd backend
   dotnet ef database update   # Apply migrations
   dotnet run
   ```

3. **Frontend**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

4. **Visit**  
   Open [http://localhost:5173](http://localhost:5173) in your browser.

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