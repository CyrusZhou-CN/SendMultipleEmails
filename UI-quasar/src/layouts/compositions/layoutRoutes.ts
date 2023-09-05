'use strict'

import { useRoute, useRouter } from 'vue-router'
import { RouteRecordRawExtend } from '../../types/routeRecordRawExtend'
import { walkTree } from 'src/utils/tree/index'

/**
 * 返回当前激活的路由
 */
export function useActivatedRoute() {
  return useRoute()
}

/**
 * 返回所有固定的路由
 */
export function useStickyRoutes() {
  const allRoutes = useAllRoutes()
  return allRoutes.filter((x) => x.meta?.sticky)
}

/**
 * 所有的路由
 * @returns
 */
export function useAllRoutes() {
  const router = useRouter()
  return [...router.options.routes] as RouteRecordRawExtend[]
}

/**
 * 获取所有的叶子路由
 * @returns
 */
export function useAllLeafRoutes() {
  const allRoutes = useAllRoutes()

  const results: RouteRecordRawExtend[] = []
  walkTree(allRoutes, (route) => {
    if (route.meta?.hidden) return

    if (!route.children || route.children.length === 0) {
      results.push(route)
    }
  })

  return results
}
