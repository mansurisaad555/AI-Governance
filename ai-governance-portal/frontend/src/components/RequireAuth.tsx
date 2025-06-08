import React, { type JSX } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useUser } from '../context/UserContext';

const RequireAuth: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { user } = useUser();
  const location = useLocation();
  if (!user) {
    // redirect to login, preserve attempted URL
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
  return children;
};

export default RequireAuth;
