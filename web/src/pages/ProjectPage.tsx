import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { api } from '../api/client';

interface Service {
  id: string;
  name: string;
  status: string;
}

export default function ProjectPage() {
  const { orgId, projectId } = useParams<{ orgId: string; projectId: string }>();
  const [services, setServices] = useState<Service[]>([]);
  const [projectName, setProjectName] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    if (!orgId || !projectId) return;
    api.getProject(orgId, projectId)
      .then((p) => setProjectName(p.name))
      .catch((err) => setError(err.message));
    api.listServices(projectId)
      .then(setServices)
      .catch((err) => setError(err.message));
  }, [orgId, projectId]);

  return (
    <div style={{ maxWidth: 800, margin: '2rem auto', fontFamily: 'sans-serif' }}>
      <Link to={`/orgs/${orgId}`}>← Organization</Link>
      <h1>{projectName || 'Project'}</h1>
      <h2>Services</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {services.length === 0 && !error && <p>No services found.</p>}
      <ul>
        {services.map((s) => (
          <li key={s.id}>
            {s.name} — <strong>{s.status}</strong>
          </li>
        ))}
      </ul>
    </div>
  );
}
