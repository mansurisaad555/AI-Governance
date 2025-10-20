// src/pages/Dashboard.tsx
import React, { useEffect, useState } from 'react';
import {
  Container,
  Table,
  Badge,
  Button,
  Spinner,
  Alert,
  Form,
  OverlayTrigger,
  Tooltip,
  ButtonGroup
} from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import axios from '../api/axios';
import { useUser } from '../context/UserContext';
import ModelCardModal, { type ModelCard } from '../components/ModelCardModal';

const buildUpdatePayload = (entry: Entry) => ({
  username: entry.username,
  toolName: entry.toolName,
  dataType: entry.dataType,
  purpose: entry.purpose,
  frequency: entry.frequency,
  riskLevel: entry.riskLevel,
  status: entry.status,
  denialReason: entry.denialReason,
});

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
  aiRiskLevel: string;
  aiConfidence?: number;
  aiRecommendation?: string;
  aiRationale?: string;
  complianceChecklist: string;
  policyAlerts?: string;
  adversarialFlag: boolean;
  adversarialIndicators?: string;
  majorViolations?: string;
  denialReason?: string;
  autoDecisionSource?: string;
  modelCard?: ModelCard | null;
}

const Dashboard: React.FC = () => {
  const { user } = useUser();
  const navigate = useNavigate();
  const [entries, setEntries] = useState<Entry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedCard, setSelectedCard] = useState<ModelCard | null>(null);
  const [showCardModal, setShowCardModal] = useState(false);

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
    const payload = buildUpdatePayload({ ...entry, status: newStatus });
    axios.put<Entry>(`/api/usage/${id}`, payload)
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
            <th>AI Risk</th>
            <th>Status</th>
            <th>Compliance</th>
            <th>Security</th>
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

              <td>
                <div className="d-flex flex-column gap-1">
                  <Badge bg={e.aiRiskLevel === 'High' ? 'danger' : e.aiRiskLevel === 'Medium' ? 'warning' : 'success'}>
                    {e.aiRiskLevel}
                  </Badge>
                  {user?.role === 'admin' ? (
                    <Form.Select
                      size="sm"
                      value={e.riskLevel}
                      onChange={ev => {
                        const updatedEntry = { ...e, riskLevel: ev.target.value };
                        const payload = buildUpdatePayload(updatedEntry);
                        axios.put<Entry>(`/api/usage/${e.id}`, payload)
                          .then(res => setEntries(
                            entries.map(x => x.id === e.id ? res.data : x)
                          ))
                          .catch(err => setError(err.message));
                      }}
                    >
                      <option>Low</option>
                      <option>Medium</option>
                      <option>High</option>
                    </Form.Select>
                  ) : (
                    <span className="text-muted">Final: {e.riskLevel}</span>
                  )}
                  {typeof e.aiConfidence === 'number' && (
                    <small className="text-muted">Conf {(e.aiConfidence * 100).toFixed(1)}%</small>
                  )}
                </div>
              </td>

              <td>
                <Badge bg={e.status === 'Approved' ? 'success' : e.status === 'Denied' ? 'danger' : 'warning'}>
                  {e.status}
                </Badge>
                {e.denialReason && (
                  <div className="text-danger small mt-1">{e.denialReason}</div>
                )}
              </td>

              <td>
                <OverlayTrigger
                  placement="top"
                  overlay={
                    <Tooltip id={`compliance-${e.id}`} className="text-start">
                      {(e.complianceChecklist.split(';').map(item => item.trim()).filter(Boolean) || ['None']).map(item => (
                        <div key={item}>{item}</div>
                      ))}
                    </Tooltip>
                  }
                >
                  <Badge bg="info" text="dark" style={{ cursor: 'pointer' }}>
                    {e.aiRecommendation || 'Pending'}
                  </Badge>
                </OverlayTrigger>
                {e.policyAlerts && (
                  <div className="small text-muted mt-1">Alerts: {e.policyAlerts}</div>
                )}
              </td>

              <td>
                {e.adversarialFlag ? (
                  <Badge bg="danger">Prompt Attack</Badge>
                ) : (
                  <Badge bg="secondary">Clean</Badge>
                )}
                {e.adversarialIndicators && (
                  <div className="small text-muted">{e.adversarialIndicators}</div>
                )}
              </td>

              <td>
                <ButtonGroup size="sm">
                  <Button
                    variant="outline-primary"
                    onClick={() => navigate(`/edit/${e.id}`)}
                  >
                    Edit
                  </Button>
                  {user?.role === 'admin' && (
                    <Button
                      variant={e.status === 'Approved' ? 'secondary' : 'success'}
                      onClick={() => toggleStatus(e.id)}
                    >
                      {e.status === 'Approved' ? 'Unapprove' : 'Approve'}
                    </Button>
                  )}
                  <Button
                    variant="danger"
                    onClick={() => deleteEntry(e.id)}
                  >
                    Delete
                  </Button>
                </ButtonGroup>
                {e.modelCard && (
                  <div className="mt-2">
                    <Button
                      size="sm"
                      variant="outline-secondary"
                      onClick={() => {
                        setSelectedCard(e.modelCard ?? null);
                        setShowCardModal(true);
                      }}
                    >
                      View Model Card
                    </Button>
                  </div>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <ModelCardModal
        show={showCardModal}
        onHide={() => {
          setShowCardModal(false);
          setSelectedCard(null);
        }}
        card={selectedCard}
      />
    </Container>
  );
};

export default Dashboard;
