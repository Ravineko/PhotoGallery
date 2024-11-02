import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css'
})
export class ButtonComponent implements OnInit {
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() label: string = 'Submit';
  @Input() buttonClass: string = 'default-button';
  @Output() clickEvent = new EventEmitter<void>(); 

  ngOnInit(): void {
    if (this.type === 'submit' && this.clickEvent.observed) {
      throw new Error("Buttons type 'submit' can't have clickEvent.");
    }
  }

  handleClick(): void {
    if (this.type !== 'submit') {
      this.clickEvent.emit();
    }
  }
}
