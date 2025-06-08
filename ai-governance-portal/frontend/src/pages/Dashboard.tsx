import React, { useEffect, useState } from 'react';
import { Container, Table, Badge, Button, Spinner, Alert, Form } from 'react-bootstrap';
import axios from 'axios';
import { useUser } from '../context/UserContext';

const BACKEND = 'http://localhost:5056';

interface Entry {
  id: number;
  username: string;
  toolName: string;
  dataType: string;
  purpose: string;
  frequency: number;
  riskLevel: string;
  status: string;
  createdAt: string;
}

const Dashboard: React.FC = () => {
  const { user } = useUser();
  const [entries, setEntries] = useState<Entry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user) return;
    const url = user.role === 'admin'
      ? `${BACKEND}/api/usage`
      : `${BACKEND}/api/usage/user/${encodeURIComponent(user.name)}`;
    axios.get<Entry[]>(url)
      .then(res => setEntries(res.data))
      .catch(err => setError(err.message))
      .finally(()=>setLoading(false));
  }, [user]);

  const updateRisk = (id: number, newLevel: string) => {
    const entry = entries.find(e => e.id === id);
    if (!entry) return;
    axios.put(`${BACKEND}/api/usage/${id}`, { ...entry, riskLevel: newLevel })
      .then(res => {
        setEntries(entries.map(e => e.id === id ? res.data : e));
      })
      .catch(err => setError(err.message));
  };

  if (loading) return <Spinner animation="border" />;
  if (error) return <Alert variant="danger">{error}</Alert>;

  return (
    <Container className="py-4">
      <h2>{user?.role === 'admin' ? 'Admin Dashboard' : 'Your Submissions'}</h2>
      <Table striped bordered hover responsive>
        <thead>
          <tr>
            <th>#</th>
            <th>User</th>
            <th>Tool</th>
            <th>Data Type</th>
            <th>Purpose</th>
            <th>Freq</th>
            {user?.role==='admin' && <th>Risk</th>}
            <th>Status</th>
            {user?.role==='admin' && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {entries.map(e => (
            <tr key={e.id}>
              <td>{e.id}</td>
              <td>{e.username}</td>
              <td>{e.toolName}</td>
              <td>{e.dataType}</td>
              <td>{e.purpose}</td>
              <td>{e.frequency}</td>
              {user?.role==='admin' && (
                <td>
                  <Form.Select
                    size="sm"
                    value={e.riskLevel}
                    onChange={ev => updateRisk(e.id, ev.target.value)}
                  >
                    <option>Low</option>
                    <option>Medium</option>
                    <option>High</option>
                  </Form.Select>
                </td>
              )}
              <td><Badge bg="info">{e.status}</Badge></td>
              {user?.role==='admin' && (
                <td>
                  {/* Future: approve / flag buttons */}
                  <Button size="sm">Approve</Button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default Dashboard;
