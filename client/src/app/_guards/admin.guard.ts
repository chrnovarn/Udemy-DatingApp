import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor (private AccountService:AccountService, private toastr: ToastrService){

  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean>{
      return this.AccountService.currentUser$.pipe(
        map(user => {
          if(user.roles.includes('Admin') || user.roles.includes('Moderator'))
            return true;
          this.toastr.error('You cannot enter this area');
        })
      )
  }
  
}
