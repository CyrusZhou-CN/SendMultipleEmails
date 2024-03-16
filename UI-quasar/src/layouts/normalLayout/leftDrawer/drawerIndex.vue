<template>
  <q-drawer v-model="leftDrawerOpen" bordered :width="200">
    <q-scroll-area class="fit">
      <q-tabs v-model="defaultTabName" vertical inline-label indicator-color="primary" active-class="text-primary">
        <template v-for="groupName in routeGroupNames">
          <q-tab v-for="route in filterRoutesByGroupName(groupName)" :key="route.meta ? route.name : Date.now()"
            :icon="route.meta?.icon" :label="route.meta?.title" :name="String(route.name)" @click="goRoute(route)" />
          <q-separator :key="groupName" v-if="!isLastGroup(groupName)" inset />
        </template>
      </q-tabs>
    </q-scroll-area>
  </q-drawer>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import { useAllLeafRoutes, useActivatedRoute } from '../../compositions/layoutRoutes'
import { useRouter, type RouteRecordRaw } from 'vue-router'

import { leftDrawerOpen, defaultTabName } from './useDrawerProp'

/**
 * 抽屉分类
 */
const allRoutes = useAllLeafRoutes()
// 将 allRoutes 按 meta.group 字符串进行升序
allRoutes.sort((a, b) => {
  if (a.meta?.group && b.meta?.group) {
    return a.meta.group.localeCompare(b.meta.group)
  } else {
    return 0
  }
})
// 获取所有的分类
const routeGroupNames = computed(() => {
  const groupNames = new Set(allRoutes.map(route => route.meta?.group).filter(group => group))
  return Array.from(groupNames)
})
function filterRoutesByGroupName(groupName: string | undefined) {
  return allRoutes.filter(route => route.meta?.group === groupName)
}
// 是否是最后一个分类
function isLastGroup(groupName: string) {
  return routeGroupNames.value.indexOf(groupName) === routeGroupNames.value.length - 1
}

/**
 * 当前激活的路由
 */
const activatedRoute = computed(() => useActivatedRoute())
// 将当前路由的名称设置为默认的 tab 名称
defaultTabName.value = activatedRoute.value.name?.toString() || ''

const router = useRouter()
function goRoute(route: RouteRecordRaw) {
  router.push({
    name: route.name
  })
}
</script>

<style lang="scss" scoped></style>
