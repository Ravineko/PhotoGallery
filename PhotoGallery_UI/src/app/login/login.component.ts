import { Component } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../core/services/authorize.service';
import { Router, RouterModule } from '@angular/router';
import { BehaviorSubject} from 'rxjs';
import { FormControlComponent } from '../shared/form-control/form-control.component';
import { ButtonComponent } from '../shared/button/button.component';
import { ToastrService } from 'ngx-toastr';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormControlComponent,
    ButtonComponent,
    RouterModule,
    AsyncPipe,
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  public message = new BehaviorSubject<string | null | undefined>(null);
  loginForm: FormGroup;

  emailErrorMessages = [
    { key: 'required', message: 'Email required' },
    { key: 'maxlength', message: 'Email maxlength' },
  ];
  
  passwordErrorMessages = [
    { key: 'required', message: 'Password required' },
    { key: 'maxlength', message: 'Password maxlength' },
  ];

  constructor(
    private authService: AuthService,
    private router: Router,
    private formBuilder: FormBuilder,
    private toastr: ToastrService
  ) {
    this.loginForm = this.formBuilder.group({
      Email: ['', [Validators.required]],
      password: ['', Validators.required],
    });
  }

  private handleLoginResult(result: any) {
    console.log(result.message);
    switch (result?.status) {
      case 'Success':
        this.toastr.success('Login Success', 'Login.successTitle');
        this.router.navigate(['/']);
        break;
      case 'Fail':
        const errorMessage =
          result.message === 'Invalid Credentials'
            ? 'Login incorrect Credentials'
            : result.message;
        this.toastr.error(errorMessage, 'Login.errorTitle');
        this.message.next(errorMessage);
        break;
      case 'TwoFactorRequired':
        this.toastr.info('2FA required', 'Login.twoFactorRequiredTitle');
        break;
      default:
        this.toastr.error('Login Unexpected Error', 'Login.errorTitle');
        this.message.next('Login Unexpected Error');
    }
  }

  private handleLoginError(error: any) {
    console.error('Error during Login:', error);
    this.toastr.error('Login.genericErrorMessage', 'Login.genericErrorTitle');
    this.message.next('Login.genericErrorMessage');
  }

  getControl(controlName: string): FormControl {
    return this.loginForm.get(controlName) as FormControl;
  }

  handleSubmit() {
    if (!this.loginForm.valid) {
      this.toastr.error('Login.invalidFormMessage', 'Login.invalidFormTitle');
      return;
    }
    const email = this.loginForm.get('Email')!.value;
    const password = this.loginForm.get('password')!.value;

    this.authService.login(email, password).subscribe({
      next: (result) => this.handleLoginResult(result),
      error: (error) => this.handleLoginError(error),
    });
  }
}
