import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, NgForm, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from 'src/services/api.service';
import { Usuario } from 'src/model/Usuario';

@Component({
  selector: 'login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  username: String = '';
  password: String = '';
  dataSource: Usuario = new Usuario();
  isLoadingResults = false;

  constructor(private router: Router, private api: ApiService,
     private formBuilder: FormBuilder) { }

  ngOnInit() {
     this.loginForm = this.formBuilder.group({
    'username' : [null, Validators.required],
    'password' : [null, Validators.required]
  });
  }

  addLogin(form: NgForm) {
    this.isLoadingResults = true;
    this.api.Login(form)
      .subscribe(res => {
          this.dataSource = res;
          localStorage.setItem("jwt", this.dataSource.token);
          this.isLoadingResults = false;
          this.router.navigate(['/categorias']);
        }, (err) => {
          console.log(err);
          this.isLoadingResults = false;
        });
  }
}