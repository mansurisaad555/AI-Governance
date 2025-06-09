// src/pages/UsageForm.tsx
import React, { useState, useEffect } from 'react';
import { Form, Button, Container, Card, Alert, Spinner } from 'react-bootstrap';
import axios from '../api/axios';
import { useUser } from '../context/UserContext';
import { useNavigate, useParams } from 'react-router-dom';

interface UsageEntry {
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

const UsageForm: React.FC = () => {
  const { user } = useUser();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();          // read :id
  const isEdit = Boolean(id);

  // form state
  const [tool, setTool] = useState('');
  const [dataType, setDataType] = useState('');
  const [purpose, setPurpose] = useState('');
  const [frequency, setFrequency] = useState<number>(1);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(isEdit);

  // if edit-mode, fetch existing entry
  useEffect(() => {
    if (!isEdit) return;
    setLoading(true);
    axios.get<UsageEntry>(`/api/usage/${id}`)
      .then(res => {
        const e = res.data;
        setTool(e.toolName);
        setDataType(e.dataType);
        setPurpose(e.purpose);
        setFrequency(e.frequency);
      })
      .catch(err => {
        setError(err instanceof Error ? err.message : 'Failed to load entry');
      })
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;
    setError('');

    const payload = {
      username: user.name,
      toolName: tool,
      dataType,
      purpose,
      frequency,
      riskLevel: '',                 // admin sets later
      status: isEdit ? undefined : 'Pending'
    };

    try {
      if (isEdit && id) {
        await axios.put(`/api/usage/${id}`, payload);
      } else {
        await axios.post('/api/usage', payload);
      }
      navigate('/dashboard');
    } catch (err: unknown) {
      if (err instanceof Error) setError(err.message);
      else setError('Failed to save');
    }
  };

  if (loading) {
    return (
      <Container className="py-4 text-center">
        <Spinner animation="border" />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Card>
        <Card.Header>
          {isEdit ? 'Edit AI Usage Entry' : 'New AI Usage Entry'}
        </Card.Header>
        <Card.Body>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Tool Name</Form.Label>
              <Form.Control
                value={tool}
                onChange={e => setTool(e.target.value)}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Data Type</Form.Label>
              <Form.Select
                value={dataType}
                onChange={e => setDataType(e.target.value)}
                required
              >
                <option value="">Select…</option>
                <option>Public</option>
                <option>Internal-only</option>
                <option>Customer/PII</option>
                <option>Financial/IP</option>
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Purpose</Form.Label>
              <Form.Control
                as="textarea"
                rows={2}
                value={purpose}
                onChange={e => setPurpose(e.target.value)}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Frequency (per week)</Form.Label>
              <Form.Control
                type="number"
                min={1}
                value={frequency}
                onChange={e => setFrequency(+e.target.value)}
                required
              />
            </Form.Group>

            <Button type="submit">
              {isEdit ? 'Save Changes' : 'Submit'}
            </Button>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default UsageForm;
