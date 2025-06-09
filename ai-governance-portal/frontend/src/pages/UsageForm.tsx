import React, { useState } from 'react';
import { Form, Button, Container, Card, Alert } from 'react-bootstrap';
import axios from '../api/axios';
import { useUser } from '../context/UserContext';
import { useNavigate } from 'react-router-dom';

const UsageForm: React.FC = () => {
  const { user } = useUser();
  const navigate = useNavigate();
  const [tool, setTool] = useState('');
  const [dataType, setDataType] = useState('');
  const [purpose, setPurpose] = useState('');
  const [frequency, setFrequency] = useState<number>(1);
  const [error, setError] = useState<string>('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;
    try {
      await axios.post('/api/usage', {
        username: user.name,
        toolName: tool,
        dataType,
        purpose,
        frequency,
        riskLevel: '',      
        status: 'Pending', 
      });
      navigate('/dashboard');
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Submission failed');
    }
  };

  return (
    <Container className="py-4">
      <Card>
        <Card.Header>New AI Usage Entry</Card.Header>
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
            <Button type="submit">Submit</Button>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default UsageForm;
