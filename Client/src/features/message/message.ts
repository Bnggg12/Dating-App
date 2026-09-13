import { Component, inject, signal } from '@angular/core';
import { MessageService } from '../../core/services/message-service';
import { Router } from '@angular/router';
import { PaginatedResult } from '../../types/pagination';
import { Message as MessageType } from '../../types/message';
import { TimeAgoPipe } from '../../core/pipes/time-ago-pipe';
import { PresenceService } from '../../core/services/presence-service';

@Component({
  selector: 'app-message',
  imports: [TimeAgoPipe],
  templateUrl: './message.html',
  styleUrl: './message.css',
})
export class Message {
  private messageService = inject(MessageService);
  protected presenceService = inject(PresenceService);
  private router = inject(Router);

  protected messages = signal<MessageType[]>([]);
  protected pagination = signal<PaginatedResult<MessageType> | null>(null);
  protected container = signal<'Inbox' | 'Outbox'>('Inbox');
  protected pageNumber = 1;
  protected pageSize = 10;
  protected pages = signal<number[]>([]);

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessages(this.container(), this.pageNumber, this.pageSize).subscribe({
      next: (res) => {
        this.messages.set(res.items);
        this.pagination.set(res);

        const total = res.totalPages ?? 0;
        this.pages.set(Array.from({ length: total }, (_, i) => i + 1));
      },
    });
  }

  setContainer(type: 'Inbox' | 'Outbox') {
    this.container.set(type);
    this.pageNumber = 1;
    this.loadMessages();
  }

  pageChanged(page: number) {
    if (this.pageNumber !== page) {
      this.pageNumber = page;
      this.loadMessages();
    }
  }

  navigateToChat(message: MessageType) {
    const targetUserId = this.container() === 'Outbox' ? message.recipientId : message.senderId;
    this.router.navigate(['/members', targetUserId, 'messages']);
  }

  deleteMessage(event: Event, id: number) {
    event.stopPropagation();
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        this.messages.update((msgs) => msgs.filter((m) => m.id !== id));
      },
    });
  }
}
