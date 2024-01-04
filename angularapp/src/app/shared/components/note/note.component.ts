import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss'],
})
export class NoteComponent {
  private _expansionPanel = false;

  @Input()
  set expansionPanel(value: any) {
    this._expansionPanel = value !== null && `${value}` !== 'false';
  }
  @Input() inputText: string = '';

  get expansionPanel() {
    return this._expansionPanel;
  }

  showExpansionPanelIfExists: boolean = true;

  constructor() {}

  removeNote(): void {
    this.showExpansionPanelIfExists = false;
  }
}
