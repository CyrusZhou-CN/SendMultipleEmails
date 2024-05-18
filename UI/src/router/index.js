import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/layout'

export const constantRoutes = [
  {
    path: '/login',
    component: () => import('@/views/login/index'),
    hidden: true
  },
  {
    path: '/404',
    component: () => import('@/views/404'),
    hidden: true
  },
  {
    path: '/',
    component: Layout,
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/index'),
        meta: { title: '首页', icon: 'dashboard' }
      }
    ]
  },

  {
    path: '/profile',
    component: Layout,
    redirect: '/index',
    children: [
      {
        path: 'index',
        name: 'Profile',
        hidden: true,
        component: () => import('@/views/profile/index'),
        meta: { title: '我的资料', icon: 'dashboard' }
      }
    ]
  },

  {
    path: '/setting',
    component: Layout,
    children: [
      {
        path: 'index',
        name: 'Setting',
        component: () => import('@/views/setting/index'),
        meta: { title: '系统设置', icon: 'setting' }
      }
    ]
  },

  {
    path: '/email',
    component: Layout,
    meta: { title: '邮箱管理', icon: 'data' },
    redirect: '/email/send-box',
    children: [
      {
        path: 'send-box',
        name: 'SendBox',
        component: () => import('@/views/email/send'),
        meta: { title: '发件箱', icon: 'outbox', noCache: false }
      },
      {
        path: 'receive-box',
        name: 'ReceiveBox',
        component: () => import('@/views/email/receive'),
        meta: { title: '收件箱', icon: 'inbox', noCache: false }
      },
      {
        path: 'template',
        name: 'Template',
        component: () => import('@/views/email/template'),
        meta: { title: '正文模板', icon: 'template-f' }
      },
      {
        path: 'template-editor',
        name: 'TemplateEditor',
        hidden: true,
        component: () => import('@/views/email/templateEditor'),
        meta: { title: '编辑模板', icon: 'form' }
      }
    ]
  },

  {
    path: '/send',
    component: Layout,
    meta: { title: '发件管理', icon: 'send' },
    redirect: '/send/index',
    children: [
      {
        path: 'index',
        name: 'SendIndex',
        component: () => import('@/views/send/index'),
        meta: { title: '新建发件', icon: 'add' }
      },
      {
        path: 'history',
        name: 'SendHistory',
        component: () => import('@/views/send/history'),
        meta: { title: '发件历史', icon: 'history' }
      }
    ]
  },

  {
    path: 'readme',
    component: Layout,
    children: [
      {
        path: 'https://galensgan.github.io/posts/2020/2QMK677.html',
        meta: { title: '使用说明', icon: 'book' }
      }
    ]
  },

  // 404 page must be placed at the end !!!
  { path: '*', redirect: '/404', hidden: true }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes: constantRoutes,
})

export function resetRouter() {
  const newRouter = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes: constantRoutes,
  })
  router.matcher = newRouter.matcher // 重置路由器
}

export default router
