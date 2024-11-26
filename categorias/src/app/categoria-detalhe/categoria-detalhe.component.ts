import { Component, OnInit } from '@angular/core';
import { Categoria } from 'src/model/Categoria';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/services/api.service';

@Component({
  selector: 'app-categoria-detalhe',
  templateUrl: './categoria-detalhe.component.html',
  styleUrls: ['./categoria-detalhe.component.scss']
})
export class CategoriaDetalheComponent implements OnInit {
  categoria: Categoria = { id: '', nome: '', imagemUrl: ''};
  isLoadingResults = true;
  constructor(private router: Router, private route: ActivatedRoute, private api: ApiService) { }

  ngOnInit() {
    this.getCategoria(this.route.snapshot.params['id']);
  }

  getCategoria(id: string) {
    this.api.getCategoria(id)
      .subscribe(data => {
        this.categoria = data;
           console.log(this.categoria);
             this.isLoadingResults = false;
      });
  }

  deleteCategoria(id: string) {
    this.isLoadingResults = true;
    this.api.deleteCategoria(id)
      .subscribe(res => {
          this.isLoadingResults = false;
          this.router.navigate(['/categorias']);
        }, (err) => {
          console.log(err);
          this.isLoadingResults = false;
        }
      );
  }
}