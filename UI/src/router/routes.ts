import { RouteRecordRaw } from 'vue-router'

const normalLayout = () => import('src/layouts/normalLayout/index.vue')

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: normalLayout,
    children: [{ path: '', component: () => import('pages/IndexPage.vue') }]
  },

  {
    path: '/outbox',
    component: normalLayout,
    children: [{ path: '', component: () => import('pages/outbox/outboxMain.vue') }]
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue')
  }
]

export default routes
