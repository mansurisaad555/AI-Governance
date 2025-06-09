import React from 'react';
import { Navbar, Nav, Container, Button } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { useUser } from '../context/UserContext';

const HomeNav: React.FC = () => {
  const { user, setUser } = useUser();
  const nav = useNavigate();

  const logout = () => {
    setUser(null);
    nav('/login', { replace: true });
  };

  return (
    <Navbar bg="dark" variant="dark" expand="md" className="mb-4">
      <Container>
        <Navbar.Brand>AI Governance</Navbar.Brand>
        <Navbar.Toggle aria-controls="main-nav" />
        <Navbar.Collapse id="main-nav">
          <Nav className="me-auto">
            {!user && <Nav.Link as={Link} to="/login">Login</Nav.Link>}
            {user?.role === 'employee' && (
              <Nav.Link as={Link} to="/submit">Submit Usage</Nav.Link>
            )}
            {user && (
              <Nav.Link as={Link} to="/dashboard">Dashboard</Nav.Link>
            )}
          </Nav>
          {user && (
            <Button variant="outline-light" size="sm" onClick={logout}>
              Logout
            </Button>
          )}
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default HomeNav;
