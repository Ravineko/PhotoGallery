import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function matchPasswords(passwordKey: string, confirmPasswordKey: string): ValidatorFn {
  return (formGroup: AbstractControl): ValidationErrors | null => {
    const passwordControl = formGroup.get(passwordKey);
    const confirmPasswordControl = formGroup.get(confirmPasswordKey);

    if (!passwordControl || !confirmPasswordControl) {
      return null;
    }

    if (confirmPasswordControl.errors && !confirmPasswordControl.errors['passwordMismatch']) {
      return null;
    }

    const isPasswordMismatch = passwordControl.value !== confirmPasswordControl.value;
    confirmPasswordControl.setErrors(isPasswordMismatch ? { passwordMismatch: true } : null);
    return isPasswordMismatch ? { passwordMismatch: true } : null;
    
  };
}
