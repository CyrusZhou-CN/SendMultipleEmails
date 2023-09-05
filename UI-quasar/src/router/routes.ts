import { RouteRecordRawExtend } from 'src/types/routeRecordRawExtend'

const normalLayout = () => import('src/layouts/normalLayout/normalLayout.vue')

const routes: RouteRecordRawExtend[] = [
  {
    path: '/',
    component: normalLayout,
    children: [
      {
        path: '',
        name: 'index',
        component: () => import('pages/IndexPage.vue'),
        meta: { icon: 'photo', sticky: true, title: '首页', group: 'setting' }
      }
    ]
  },
  {
    path: '/outbox',
    component: normalLayout,
    children: [
      {
        path: '',
        component: () => import('pages/outbox/outboxMain.vue'),
        name: 'outbox',
        meta: {
          icon: 'email',
          sticky: true,
          title: '发件箱',
          group: 'outbox'
        }
      }
    ]
  },
  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
    meta: {
      hidden: true
    }
  }
]

export default routes
