import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { api } from '../api/client';

interface Project {
  id: string;
  name: string;
  slug: string;
}

export default function OrgPage() {
  const { orgId } = useParams<{ orgId: string }>();
  const [projects, setProjects] = useState<Project[]>([]);
  const [orgName, setOrgName] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    if (!orgId) return;
    api.getOrg(orgId)
      .then((org) => setOrgName(org.name))
      .catch((err) => setError(err.message));
    api.listProjects(orgId)
      .then(setProjects)
      .catch((err) => setError(err.message));
  }, [orgId]);

  return (
    <div style={{ maxWidth: 800, margin: '2rem auto', fontFamily: 'sans-serif' }}>
      <Link to="/orgs">← All Organizations</Link>
      <h1>{orgName || 'Organization'}</h1>
      <h2>Projects</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {projects.length === 0 && !error && <p>No projects found.</p>}
      <ul>
        {projects.map((p) => (
          <li key={p.id}>
            <Link to={`/orgs/${orgId}/projects/${p.id}`}>{p.name}</Link> ({p.slug})
          </li>
        ))}
      </ul>
    </div>
  );
}
