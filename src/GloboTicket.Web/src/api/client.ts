import type {
  LoginResponse,
  UserInfo,
  TenantDto,
  CreateTenantDto,
} from '../types/api';
import type { Venue, CreateVenueDto, UpdateVenueDto } from '../types/venue';
import type { Act, CreateActDto, UpdateActDto } from '../types/act';

const API_BASE_URL = '';

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error: ${response.status} - ${errorText}`);
  }
  return response.json();
}

export async function login(
  username: string,
  password: string
): Promise<LoginResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify({ username, password }),
  });
  return handleResponse<LoginResponse>(response);
}

export async function logout(): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/auth/logout`, {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) {
    throw new Error(`Logout failed: ${response.status}`);
  }
}

export async function getCurrentUser(): Promise<UserInfo> {
  const response = await fetch(`${API_BASE_URL}/auth/me`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<UserInfo>(response);
}

export async function getTenants(): Promise<TenantDto[]> {
  const response = await fetch(`${API_BASE_URL}/api/tenants`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<TenantDto[]>(response);
}

export async function getTenant(id: number): Promise<TenantDto> {
  const response = await fetch(`${API_BASE_URL}/api/tenants/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<TenantDto>(response);
}

export async function createTenant(
  dto: CreateTenantDto
): Promise<TenantDto> {
  const response = await fetch(`${API_BASE_URL}/api/tenants`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<TenantDto>(response);
}

export async function getVenues(): Promise<Venue[]> {
  const response = await fetch(`${API_BASE_URL}/api/venues`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Venue[]>(response);
}

export async function getVenue(id: string): Promise<Venue> {
  const response = await fetch(`${API_BASE_URL}/api/venues/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Venue>(response);
}

export async function createVenue(dto: CreateVenueDto): Promise<Venue> {
  const response = await fetch(`${API_BASE_URL}/api/venues`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });

  return handleResponse<Venue>(response);
}

export async function updateVenue(id: string, dto: UpdateVenueDto): Promise<Venue> {
  const response = await fetch(`${API_BASE_URL}/api/venues/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });

  return handleResponse<Venue>(response);
}

export async function deleteVenue(id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/venues/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error: ${response.status} - ${errorText}`);
  }
}

export async function getActs(): Promise<Act[]> {
  const response = await fetch(`${API_BASE_URL}/api/acts`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Act[]>(response);
}

export async function getAct(id: string): Promise<Act> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${id}`, {
    method: 'GET',
    credentials: 'include',
  });
  return handleResponse<Act>(response);
}

export async function createAct(dto: CreateActDto): Promise<Act> {
  const response = await fetch(`${API_BASE_URL}/api/acts`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<Act>(response);
}

export async function updateAct(id: string, dto: UpdateActDto): Promise<Act> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(dto),
  });
  return handleResponse<Act>(response);
}

export async function deleteAct(id: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/acts/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error: ${response.status} - ${errorText}`);
  }
}