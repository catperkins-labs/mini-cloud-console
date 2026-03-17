// API base URL – set VITE_API_URL in your .env.local to override
export const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080';

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token');
  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
  });

  if (!res.ok) {
    throw new Error(`API error ${res.status}: ${res.statusText}`);
  }

  return res.json() as Promise<T>;
}

export const api = {
  devLogin: (email: string) =>
    request<{ token: string }>('/auth/dev-login', {
      method: 'POST',
      body: JSON.stringify({ email }),
    }),

  listOrgs: () => request<{ id: string; name: string; slug: string }[]>('/orgs'),

  getOrg: (orgId: string) =>
    request<{ id: string; name: string; slug: string }>(`/orgs/${orgId}`),

  listProjects: (orgId: string) =>
    request<{ id: string; name: string; slug: string }[]>(`/orgs/${orgId}/projects`),

  getProject: (orgId: string, projectId: string) =>
    request<{ id: string; name: string; slug: string }>(
      `/orgs/${orgId}/projects/${projectId}`
    ),

  listServices: (projectId: string) =>
    request<{ id: string; name: string; status: string }[]>(
      `/projects/${projectId}/services`
    ),

  listOrgMembers: (orgId: string) =>
    request<{ id: string; email: string; displayName: string; role: string }[]>(
      `/orgs/${orgId}/members`
    ),
};
