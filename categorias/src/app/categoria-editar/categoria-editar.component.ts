import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, NgForm, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from 'src/services/api.service';

@Component({
  selector: 'app-categoria-editar',
  templateUrl: './categoria-editar.component.html',
  styleUrls: ['./categoria-editar.component.scss']
})
export class CategoriaEditarComponent implements OnInit {
  id: String = '';
  categoriaForm!: FormGroup;
  nome: String = '';
  imagemUrl: String = '';

  isLoadingResults = false;
  constructor(private router: Router, private route: ActivatedRoute,
     private api: ApiService, private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.getCategoria(this.route.snapshot.params['id']);
    this.categoriaForm = this.formBuilder.group({
   'id' : [null],  
   'nome' : [null, Validators.required],
   'imagemUrl' : [null, Validators.required]
 });
 }

 getCategoria(id: string) {
  this.api.getCategoria(id).subscribe(data => {
    this.id = data.id;
    this.categoriaForm.setValue({
      id: data.id,
      nome: data.nome,
      imagemUrl : data.imagemUrl,
    });
  });
}

updateCategoria(form: NgForm) {
  this.isLoadingResults = true;
  this.api.updateCategoria(this.id, form)
    .subscribe(res => {
        this.isLoadingResults = false;
        this.router.navigate(['/categoria-detalhe/' + this.id]);
      }, (err) => {
        console.log(err);
        this.isLoadingResults = false;
      }
    );
 }
}