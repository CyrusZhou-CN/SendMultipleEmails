import { RouteRecordRaw } from 'vue-router'

/**
 * 扩展的路由结果
 */
export type RouteRecordRawExtend = RouteRecordRaw & {
  meta: {
    title: string
    icon: string
    sticky: boolean
    group: string
  }
}
