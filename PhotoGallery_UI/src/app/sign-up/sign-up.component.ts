import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from '../core/services/authorize.service';
import { FormControlComponent } from '../shared/form-control/form-control.component';
import { ButtonComponent } from '../shared/button/button.component';
import { matchPasswords } from '../core/validators/match-password.validator';
import { ToastrService } from 'ngx-toastr';
import { AsyncPipe } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormControlComponent,
    ButtonComponent,
    AsyncPipe,
  ],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css',
})
export class SignUpComponent implements OnInit {
  public message = new BehaviorSubject<string | null | undefined>(null);
  registerForm!: FormGroup;

  userNameErrorMessages= [
    { key: 'required', message: 'Email required' },
    { key: 'maxlength', message: 'Email maxlength' },
  ];
  emailErrorMessages = [
    { key: 'required', message: 'Email required' },
    { key: 'maxlength', message: 'Email maxlength' },
  ];
  passwordErrorMessages= [
    { key: 'required', message: 'Email required' },
    { key: 'maxlength', message: 'Email maxlength' },
  ];
  confirmPasswordErrorMessages= [
    { key: 'required', message: 'Email required' },
    { key: 'maxlength', message: 'Email maxlength' },
  ];
  phoneNumberErrorMessages= [
    { key: 'required', message: 'Email required' },
    { key: 'maxlength', message: 'Email maxlength' },
  ];

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.registerForm = this.formBuilder.group(
      {
        userName: ['', [Validators.required, Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
        phoneNumber: [
          '',
          [Validators.required, Validators.pattern(/^[0-9]{10}$/)],
        ],
      },
      {
        validators: matchPasswords('password', 'confirmPassword'),
      }
    );
  }

  handleSubmit() {
    if (this.registerForm.valid) {
      const userName = this.registerForm.get('userName')!.value;
      const email = this.registerForm.get('email')!.value;
      const password = this.registerForm.get('password')!.value;
      const phoneNumber = this.registerForm.get('phoneNumber')!.value;

      this.authService
        .register(userName, email, password, phoneNumber)
        .subscribe({
          next: (response) => {
            if (response.status === 'Created') {
              this.toastr.success('SignUp success', 'SignUp success');
              this.registerForm.reset();
              this.router.navigate(['/login']);
            } else {
              this.message.next(response.message);
              this.registerForm.reset();
            }
          },
          error: (error) => {
            this.toastr.error('SignUp error', 'SignUp error');

            this.message.next('SignUp.errorMessage') + error;
          },
        });
    }
  }
}
