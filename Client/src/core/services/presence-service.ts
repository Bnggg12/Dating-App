import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { AccountService } from './account-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  hubConnection?: HubConnection;

  onlineUsers = signal<number[]>([]);

  createHubConnection(token: string) {
    if (this.hubConnection?.state === HubConnectionState.Connected) return;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((err) => console.error('Lỗi kết nối PresenceHub:', err));

    // Lấy toàn bộ danh sách online ban đầu
    this.hubConnection.on('GetOnlineUsers', (userIds: number[]) => {
      this.onlineUsers.set(userIds);
    });

    // Có người vừa online
    this.hubConnection.on('UserOnline', (userId: number) => {
      this.onlineUsers.update((users) => [...users, userId]);
    });

    // Có người vừa offline
    this.hubConnection.on('UserOffline', (userId: number) => {
      this.onlineUsers.update((users) => users.filter((id) => id !== userId));
    });

    // Nhận thông báo có tin nhắn mới khi đang ở trang khác
    this.hubConnection.on('NewMessageReceived', () => {
      
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((err) => console.error('Lỗi ngắt kết nối PresenceHub:', err));
    }
  }
}
