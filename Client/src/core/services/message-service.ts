import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Message, MessageCreate } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { tap } from 'rxjs';
import { AccountService } from './account-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private hubUrl = environment.hubUrl;
  private accountService = inject(AccountService);

  private hubConnection?: HubConnection;
  messageThread = signal<Message[]>([]);

  createHubConnection(otherUserId: number) {
    const user = this.accountService.currentUser();
    if (!user) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?userId=' + otherUserId, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((err) => console.error('Lỗi kết nối MessageHub:', err));

    this.hubConnection.on('ReceiveMessageThread', (messages: Message[]) => {
      this.messageThread.set(messages);
    });

    this.hubConnection.on('MessagesMarkedRead', (readerId: number) => {
      this.messageThread.update(messages => messages.map(msg => {
        if (msg.recipientId === readerId && !msg.dateRead) {
          return { ...msg, dateRead: new Date().toISOString() };
        }
        return msg;
      }));
    });

    this.hubConnection.on('NewMessage', (message: Message) => {
      this.messageThread.update((messages) => [...messages, message]);
      if (message.senderId === otherUserId) this.getMessageThread(otherUserId).subscribe(() => {
        this.hubConnection?.invoke('UpdateRead', otherUserId);
      });
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((err) => console.error('Lỗi ngắt kết nối MessageHub:', err));
    }
  }

  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams()
      .set('container', container)
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PaginatedResult<Message>>(this.baseUrl + 'message', {params});
  }

  getMessageThread(recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + recipientId).pipe(
      tap((messages) => this.messageThread.set(messages))
    );
  }

  async sendMessage(recipientId: number, content: string) {
    return this.hubConnection?.invoke('SendMessage', { recipientId, content });
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'message/' + id);
  }
}