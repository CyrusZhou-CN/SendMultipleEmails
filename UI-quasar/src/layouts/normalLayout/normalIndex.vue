<template>
  <q-layout view="hhh lpr fff" class="bg-grey-1 normal-layout ">
    <LayoutHeader v-model:leftDrawerOpen="leftDrawerOpen" />
    <LeftDrawer :leftDrawerOpen="leftDrawerOpen" />
    <!-- <PageContainer /> -->
  </q-layout>
</template>

<script lang="ts" setup>
import { ref } from 'vue'

import LayoutHeader from './header/headerIndex.vue'
import LeftDrawer from './leftDrawer/drawerIndex.vue'
import PageContainer from './pageContainer/pageIndex.vue'

const leftDrawerOpen = ref(false)

</script>

<style lang="scss">
.normal-layout {
  .q-tab__label {
    width: 60px;
    text-align: left;
  }
}
</style>
