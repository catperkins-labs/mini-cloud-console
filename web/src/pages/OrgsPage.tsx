import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api/client';

interface Org {
  id: string;
  name: string;
  slug: string;
}

export default function OrgsPage() {
  const [orgs, setOrgs] = useState<Org[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    api.listOrgs()
      .then(setOrgs)
      .catch((err) => setError(err.message));
  }, []);

  return (
    <div style={{ maxWidth: 800, margin: '2rem auto', fontFamily: 'sans-serif' }}>
      <h1>Organizations</h1>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {orgs.length === 0 && !error && <p>No organizations found.</p>}
      <ul>
        {orgs.map((org) => (
          <li key={org.id}>
            <Link to={`/orgs/${org.id}`}>{org.name}</Link> ({org.slug})
          </li>
        ))}
      </ul>
    </div>
  );
}
