export interface UserSummary {
  id: string;
  username: string;
  name: string;
  email: string | null;
  isAdmin: boolean;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserSummary;
}

export interface AdUserRow {
  username: string;
  name: string;
  email: string | null;
  portalUserId: string | null;
}

export interface PermissionMatrix {
  userId: string;
  username: string;
  sectors: { sectorId: string; sectorName: string; canView: boolean }[];
}

export interface SectorDto {
  id: string;
  name: string;
}
