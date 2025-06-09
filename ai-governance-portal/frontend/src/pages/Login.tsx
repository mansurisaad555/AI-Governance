import React, { useState, type FormEvent } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Form, Button, Container, Card } from 'react-bootstrap';
import { useUser, type Role } from '../context/UserContext';

interface LocationState { from?: { pathname: string } }

const Login: React.FC = () => {
  const { setUser } = useUser();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState<Role>('employee');
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as LocationState | undefined;
  const from = state?.from?.pathname ?? (role === 'admin' ? '/dashboard' : '/submit');

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    if (!email || !password) return;
    setUser({ name: email.split('@')[0], email, role });
    navigate(from, { replace: true });
  };

  return (
    <Container className="d-flex justify-content-center align-items-center" style={{ minHeight: '80vh' }}>
      <Card style={{ width: 360 }}>
        <Card.Header className="text-center">Sign In</Card.Header>
        <Card.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control type="email" value={email} onChange={e => setEmail(e.target.value)} required/>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Password</Form.Label>
              <Form.Control type="password" value={password} onChange={e => setPassword(e.target.value)} required/>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Role</Form.Label>
              <div>
                <Form.Check inline label="Employee" name="role" type="radio"
                  id="role-employee" checked={role==='employee'} onChange={()=>setRole('employee')} />
                <Form.Check inline label="Admin" name="role" type="radio"
                  id="role-admin" checked={role==='admin'} onChange={()=>setRole('admin')} />
              </div>
            </Form.Group>
            <Button variant="primary" type="submit" className="w-100">Login</Button>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Login;
