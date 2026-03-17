import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import OrgsPage from './pages/OrgsPage';
import OrgPage from './pages/OrgPage';
import ProjectPage from './pages/ProjectPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/orgs" element={<OrgsPage />} />
        <Route path="/orgs/:orgId" element={<OrgPage />} />
        <Route path="/orgs/:orgId/projects/:projectId" element={<ProjectPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

