import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import HomeNav from './components/HomeNav';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import UsageForm from './pages/UsageForm';
import RequireAuth from './components/RequireAuth';

const App: React.FC = () => (
  <>
    <HomeNav />
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/submit" element={<RequireAuth><UsageForm/></RequireAuth>} />
      <Route path="/dashboard" element={<RequireAuth><Dashboard/></RequireAuth>} />
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  </>
);

export default App;
