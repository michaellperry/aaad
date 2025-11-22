export interface TenantDto {
  id: number;
  name: string;
  slug: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateTenantDto {
  name: string;
  slug: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  tenantId: number;
  username: string;
}

export interface UserInfo {
  username: string;
  tenantId: number;
}