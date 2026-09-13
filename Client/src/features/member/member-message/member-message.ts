import { Component, effect, ElementRef, inject, model, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { AccountService } from '../../../core/services/account-service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-message',
  imports: [DatePipe, FormsModule],
  templateUrl: './member-message.html',
  styleUrl: './member-message.css',
})
export class MemberMessage implements OnInit, OnDestroy {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;
  
  protected messageService = inject(MessageService);
  protected accountService = inject(AccountService);
  protected presenceService = inject(PresenceService);
  private route = inject(ActivatedRoute);

  protected recipientId = signal<number>(0);
  protected messageContent = model('');

  constructor() {
    effect(() => {
      const msgs = this.messageService.messageThread();
      if (msgs.length > 0) {
        this.scrollToBottom();
      }
    });
  }

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe({
      next: (params) => {
        const id = Number(params.get('id'));
        if (id) {
          this.recipientId.set(id);
          this.loadMessages(id);
          this.messageService.createHubConnection(id);
        }
      },
    });
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  loadMessages(targetId: number) {
    this.messageService.getMessageThread(targetId).subscribe();
  }

  async sendMessage() {
    const targetId = this.recipientId();
    const content = this.messageContent().trim();
    if (!targetId || !content) return;

    try {
      await this.messageService.sendMessage(targetId, content);
      this.messageContent.set('');
    } catch (error) {
      console.error('Không thể gửi tin nhắn qua SignalR:', error);
    }
  }

  scrollToBottom() {
    setTimeout(() => {
      if (this.messageEndRef) {
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    }, 100);
  }
}
