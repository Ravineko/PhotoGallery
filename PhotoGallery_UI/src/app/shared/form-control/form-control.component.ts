import { AsyncPipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

interface ValidationError {
  key: string;
  message: string | Observable<string>;
}

@Component({
  selector: 'app-form-control',
  standalone: true,
  imports: [ReactiveFormsModule, AsyncPipe],
  templateUrl: './form-control.component.html',
  styleUrls: ['./form-control.component.css'],
})
export class FormControlComponent {
  @Input() label!: string;
  @Input() formGroup!: FormGroup;
  @Input() controlName!: string;
  @Input() isTextarea: boolean = false;
  @Input() inputType: string = 'text';
  @Input() errorMessages: ValidationError[] = [];
  @Input() inputClass: string = 'form-group';

  get control() {
    return this.formGroup.get(this.controlName);
  }

  get showError(): boolean {
    return (
      !!this.control &&
      this.control.invalid &&
      (this.control.dirty || this.control.touched)
    );
  }

  getErrorMessage(): string {
    if (!this.showError || !this.control?.errors) {
      return '';
    }

    const errors = this.control.errors;

    const errorKey = Object.keys(errors).find(
      (key) =>
        errors.hasOwnProperty(key) &&
        this.errorMessages.some((err) => err.key === key)
    );

    if (!errorKey) return '';

    const error = this.errorMessages.find((err) => err.key === errorKey);
    if (!error) return '';

    if (typeof error.message === 'string') {
      return error.message;
    }

    if (error.message instanceof Observable) {
      return this.handleObservableError(error.message);
    }

    return 'Invalid input';
  }

  private handleObservableError(message$: Observable<string>): string {
    let message = 'Error loading message';

    message$
      .pipe(catchError(() => of('Error loading message')))
      .subscribe((msg) => (message = msg));

    return message;
  }
}
