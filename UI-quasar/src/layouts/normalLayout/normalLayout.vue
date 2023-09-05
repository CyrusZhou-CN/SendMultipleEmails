<template>
  <q-layout view="lHh Lpr fff" class="bg-grey-1 normal-layout ">
    <LayoutHeader v-model:leftDrawerOpen="leftDrawerOpen" />
    <q-drawer v-model="leftDrawerOpen" bordered behavior="mobile" @click="leftDrawerOpen = false">
      <q-scroll-area class="fit">
        <q-toolbar>
          <q-toolbar-title class="row items-center text-grey-8">
            <img class="q-pl-md" src="https://www.gstatic.com/images/branding/googlelogo/svg/googlelogo_clr_74x24px.svg">
            <span class="q-ml-sm">SME</span>
          </q-toolbar-title>
        </q-toolbar>

        <q-tabs v-model="defaultTabName" vertical inline-label indicator-color="primary" active-class="text-primary">
          <template v-for="groupName in routeGroupNames">
            <q-tab v-for="route in filterRoutesByGroupName(groupName)" :key="route.meta ? route.name : Date.now()"
              :icon="route.meta?.icon" :label="route.meta?.title" :name="String(route.name)" @click="goRoute(route)" />

            <q-separator :key="groupName" v-if="!isLastGroup(groupName)" inset />
          </template>
        </q-tabs>
      </q-scroll-area>
    </q-drawer>

    <q-page-container>
      <router-view />

      <q-page-sticky v-if="$q.screen.gt.sm" expand position="left">
        <div class="fit q-pt-xl q-px-sm column">
          <q-btn v-for="stickyRoute in stickyRoutes" :key="'sticky' + stickyRoute.path" flat stack no-caps size="22px"
            @click="goRoute(stickyRoute)">
            <q-icon color="primary" size="40px" :name="stickyRoute.meta.icon" />

          </q-btn>
        </div>
      </q-page-sticky>
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue'
import LayoutHeader from './header/headerIndex.vue'
import { useAllLeafRoutes, useActivatedRoute } from '../compositions/layoutRoutes'

import { useRouter, type RouteRecordRaw } from 'vue-router'

// 右侧抽屉
const leftDrawerOpen = ref(false)

const defaultTabName = ref('')

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
function isLastGroup(groupName: string | undefined) {
  return routeGroupNames.value.indexOf(groupName) === routeGroupNames.value.length - 1
}

/**
 * 固定菜单
 */
const stickyRoutes = computed(() => {
  return allRoutes.filter(route => route.meta?.sticky)
})

/**
 * 当前激活的路由
 */
const activatedRoute = computed(() => useActivatedRoute())
function isActiveRoute(route: RouteRecordRaw) {
  return activatedRoute.value.name === route.name
}
const router = useRouter()
function goRoute(route: RouteRecordRaw) {
  console.log('goRoute', route)
  router.push({
    name: route.name
  })
}
</script>

<style lang="scss">
.normal-layout {
  .q-tab__label {
    width: 60px;
    text-align: left;
  }
}
</style>
