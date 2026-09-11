export type User = {
  id: number;
  email: string;
  displayName: string;
  imageUrl: string;
  token: string;
  roles: string[];
};

export type Login = {
  email: string;
  password: string;
};

export type Register = {
  email: string;
  password: string;
  displayName: string;
  dateOfBirth: string;
  gender: string;
  description: string;
  city: string;
};

export type UserCard = {
  id: number;
  displayName: string;
  dateOfBirth: string;
  city: string;
  gender: string;
  imageUrl: string;
};

export type UserProfile = {
  id: number
  email: string
  displayName: string
  dateOfBirth: string
  gender: string
  lookingFor: string
  description: string
  city: string
  createdAt: string
  lastActive: string
  mbti: string | null
  educationLevel: string | null
  fieldOfStudy: string | null
  institution: string | null
  interests: string[]
  photos: Photo[]
};

export type UserUpdate = {
  displayName: string;
  description: string;
  city: string;
  lookingFor: string;
  mbti?: string | null;
  educationLevel?: string | null;
  fieldOfStudy?: string | null;
  institution?: string | null;
  interests: string[];
};

export type Photo = {
  id: number
  url: string
  isMain: boolean
  isApproved: boolean
};

export type UserRole = {
  userId: number;
  email: string;
  displayName: string;
  imageUrl: string | null;
  roles: string[];
};

export type PhotoForApprove = {
  id: number;
  url: string;
  userId: number;
  displayName: string;
};