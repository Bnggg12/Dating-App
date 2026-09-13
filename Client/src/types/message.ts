export type Message = {
  id: number
  senderId: number
  senderDisplayName: string
  senderImageUrl?: string | null
  recipientId: number
  recipientDisplayName: string
  recipientImageUrl?: string | null
  content: string
  dateRead?: string | null
  messageSent: string
  currentUserSender?: boolean
}

export type MessageCreate = {
  recipientId: number;
  content: string;
};