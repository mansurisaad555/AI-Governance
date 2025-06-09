// src/pages/Dashboard.tsx
import React, { useEffect, useState } from 'react';
import {
  Container,
  Table,
  Badge,
  Button,
  Spinner,
  Alert,
  Form
} from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import axios from '../api/axios';
import { useUser } from '../context/UserContext';

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
  const navigate = useNavigate();
  const [entries, setEntries] = useState<Entry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // fetch list on mount / role change
  useEffect(() => {
    if (!user) return;
    const url =
      user.role === 'admin'
        ? '/api/usage'
        : `/api/usage/user/${encodeURIComponent(user.name)}`;

    axios.get<Entry[]>(url)
      .then(res => setEntries(res.data))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [user]);

  // toggle between Pending/Approved
  const toggleStatus = (id: number) => {
    const entry = entries.find(e => e.id === id);
    if (!entry) return;
    const newStatus = entry.status === 'Approved' ? 'Pending' : 'Approved';
    axios.put<Entry>(`/api/usage/${id}`, { ...entry, status: newStatus })
      .then(res => {
        setEntries(entries.map(e => e.id === id ? res.data : e));
      })
      .catch(err => setError(err.message));
  };

  // delete entry
  const deleteEntry = (id: number) => {
    axios.delete(`/api/usage/${id}`)
      .then(() => {
        setEntries(entries.filter(e => e.id !== id));
      })
      .catch(err => setError(err.message));
  };

  if (loading) return <Spinner animation="border" />;
  if (error)   return <Alert variant="danger">{error}</Alert>;

  return (
    <Container className="py-4">
      <h2>
        {user?.role === 'admin' ? 'Admin Dashboard' : 'Your Submissions'}
      </h2>

      <Table striped bordered hover responsive>
        <thead>
          <tr>
            <th>#</th>
            <th>User</th>
            <th>Tool</th>
            <th>Data Type</th>
            <th>Purpose</th>
            <th>Frequency</th>
            {user?.role === 'admin' && <th>Risk</th>}
            <th>Status</th>
            <th>Actions</th>
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

              {user?.role === 'admin' && (
                <td>
                  <Form.Select
                    size="sm"
                    value={e.riskLevel}
                    onChange={ev =>
                      axios.put<Entry>(`/api/usage/${e.id}`, {
                        ...e,
                        riskLevel: ev.target.value
                      })
                      .then(res => setEntries(
                        entries.map(x => x.id === e.id ? res.data : x)
                      ))
                      .catch(err => setError(err.message))
                    }
                  >
                    <option>Low</option>
                    <option>Medium</option>
                    <option>High</option>
                  </Form.Select>
                </td>
              )}

              <td>
                <Badge bg={e.status === 'Approved' ? 'success' : 'warning'}>
                  {e.status}
                </Badge>
              </td>

              <td>
                {/* Edit */}
                <Button
                  size="sm"
                  variant="outline-primary"
                  className="me-2"
                  onClick={() => navigate(`/edit/${e.id}`)}
                >
                  Edit
                </Button>

                {/* Approve/Unapprove */}
                {user?.role === 'admin' && (
                  <Button
                    size="sm"
                    variant={e.status === 'Approved' ? 'secondary' : 'success'}
                    className="me-2"
                    onClick={() => toggleStatus(e.id)}
                  >
                    {e.status === 'Approved' ? 'Unapprove' : 'Approve'}
                  </Button>
                )}

                {/* Delete */}
                <Button
                  size="sm"
                  variant="danger"
                  onClick={() => deleteEntry(e.id)}
                >
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default Dashboard;
