import { createWebHistory, createRouter,RootRecordRaw } from 'vue-router'//导入路由
import HelloWorld from '../components/HelloWorld.vue'
import MainInfo from '../components/MainInfo.vue'
import LoginInfo from '../components/LoginInfo.vue'
import MainOne from '../components/MainOne.vue'
import AppMain from '../components/AppMain.vue'
import RoleInfo from '../components/system/rolesAdmin.vue'
//定义路由
const routes = [{
    path: '/',//组件一，path和component可以乱写，
                       //只要保持path与前面to，component和import两处相同即可
    component: LoginInfo,
    name:LoginInfo


},
{
    path: '/',
    redirect: '/dashboard',
    name: 'Container',
    component: Container,
    children: [
      {path: 'dashboard', name: '首页', component: Dashboard, },
      {path: 'article', name: '文章', component: Article, },
    ]
  }

{
    path: '/Reedit',//组件一，path和component可以乱写，
                       //只要保持path与前面to，component和import两处相同即可
    component: MainInfo,
    name:MainInfo,

},
{
    path: '/HelloWorld',//组件一，path和component可以乱写，
                       //只要保持path与前面to，component和import两处相同即可
    component: HelloWorld,
    name:HelloWorld,

},
{
    path: '/MainOne',//组件一，path和component可以乱写，
                       //只要保持path与前面to，component和import两处相同即可
    component: MainOne,
    name:MainOne,

},
{
    path: '/MainInfo',//组件一，path和component可以乱写，
                       //只要保持path与前面to，component和import两处相同即可
    component: MainInfo,
    name:MainInfo,

}
,
{
    path: '/AppMain',//组件一，path和component可以乱写，
                       //只要保持path与前面to，component和import两处相同即可
    component: AppMain,
    name:AppMain,
    children: [
        {
            path: 'rolesAdmin',
            name: 'rolesAdmin',
            component: () => import('../components/system/rolesAdmin.vue'),
            meta: {
                title: '角色管理'
            }
        }
    ]

}
]
const router = createRouter({
    history: createWebHistory(),
    routes
  })

  export default router