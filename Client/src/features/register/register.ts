import { Component, inject, OnInit, output, signal } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { Router } from '@angular/router';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { TextInput } from '../../shared/text-input/text-input';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {
  private accountService = inject(AccountService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  protected cities = signal<string[]>([]);

  cancelRegister = output<boolean>();

  protected credentialForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep = signal(1);
  protected validationErrors = signal<string[]>([]);

  ngOnInit(): void {
    this.accountService.getCities().subscribe({
      next: (data) => this.cities.set(data)
    });
  }

  constructor() {
    this.credentialForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', [Validators.required, Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(32), Validators.pattern(/(?=.*[A-Z])(?=.*\d)/)]],
      confirmPassword: ['', [Validators.required, this.matchValue('password')]]
    });

    this.profileForm = this.fb.group({
      gender: ['male', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      description: ['', [Validators.required, Validators.maxLength(1000)]]
    });

    this.credentialForm.controls['password'].valueChanges.subscribe(() => {
      this.credentialForm.controls['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValue(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent;
      if (!parent) return null;
      return control.value === parent.get(matchTo)?.value ? null : { passwordMismatch: true };
    };
  }

  nextStep() {
    if (this.credentialForm.valid) this.currentStep.set(2);
  }

  prevStep() {
    this.currentStep.set(1);
  }

  getMaxDate(): string {
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];
  }

  register() {
    if (this.profileForm.valid && this.credentialForm.valid) {
      const { confirmPassword, ...creds } = this.credentialForm.value;
      const payload = { ...creds, ...this.profileForm.value };

      this.accountService.register(payload).subscribe({
        next: () => {
          this.router.navigateByUrl('/members');
          this.cancel();
        },
        error: error => {
          if (error.error?.errors) {
            const errs: string[] = [];
            for (const key in error.error.errors) {
              errs.push(...error.error.errors[key]);
            }
            this.validationErrors.set(errs);
          } else if (error.error) {
            this.validationErrors.set([error.error]);
          }
        }
      });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
