<template>
  <q-page-container>
    <div>
      <div class="row items-center text-grey-8">
        <img class="q-pl-md" src="https://www.gstatic.com/images/branding/googlelogo/svg/googlelogo_clr_74x24px.svg">
        <span class="q-ml-sm">SME</span>
      </div>
    </div>

    <q-tabs v-model="defaultTabName" vertical inline-label indicator-color="primary" active-class="text-primary">
      <template v-for="groupName in routeGroupNames">
        <q-tab v-for="route in filterRoutesByGroupName(groupName)" :key="route.meta ? route.name : Date.now()"
          :icon="route.meta?.icon" :label="route.meta?.title" :name="String(route.name)" @click="goRoute(route)" />

        <q-separator :key="groupName" v-if="!isLastGroup(groupName)" inset />
      </template>
    </q-tabs>

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
</template>

<script lang="ts" setup>
import { ref } from 'vue'
import { defaultTabName } from '../leftDrawer/useDrawerProp'

</script>

<style lang="scss" scoped></style>
