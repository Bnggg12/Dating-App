import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/member/member-list/member-list';
import { MemberDetail } from '../features/member/member-detail/member-detail';
import { MemberProfile } from '../features/member/member-profile/member-profile';
import { MemberPhoto } from '../features/member/member-photo/member-photo';
import { adminGuard } from '../core/guard/admin-guard';
import { Admin } from '../features/admin/admin';
import { authGuard } from '../core/guard/auth-guard';

export const routes: Routes = [
    { path: '', component: Home },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            { path: 'members', component: MemberList },
            { 
                path: 'members/:id', 
                runGuardsAndResolvers: 'always',
                component: MemberDetail,
                children: [
                    { path: '', redirectTo: 'profile', pathMatch: 'full' },
                    { path: 'profile', component: MemberProfile, title: 'Thông tin cá nhân',
                        // canDeactivate: [preventUnsavedChangesGuard] 
                    },
                    { path: 'photos', component: MemberPhoto, title: 'Bộ sưu tập ảnh' },
                    // { path: 'messages', component: MemberMessages, title: 'Messages' }
                ]
            },
            // { path: 'lists', component: Lists },
            // { path: 'messages', component: Messages },
            { path: 'admin', component: Admin, canActivate: [adminGuard] }
        ]
    },

    { path: '**', component: Home },
];
