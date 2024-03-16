<template>
  <q-header bordered class="bg-primary text-white" height-hint="50">
    <q-toolbar class="GPL__toolbar" style="height: 40px">
      <q-btn flat dense round @click="drawerOpenBtnClicked" aria-label="Menu" icon="menu" class="q-mx-md" />

      <q-toolbar-title v-if="$q.screen.gt.sm" shrink class="row items-center no-wrap">
        <span class="q-ml-sm">宇正群邮</span>
      </q-toolbar-title>

      <q-space />

      <div class="q-gutter-sm row items-center no-wrap">
        <q-btn round flat>
          <q-avatar size="26px">
            <img src="https://cdn.quasar.dev/img/boy-avatar.png">
          </q-avatar>
          <q-tooltip>个人信息</q-tooltip>
        </q-btn>
      </div>
    </q-toolbar>
  </q-header>
</template>

<script lang="ts" setup>
import { leftDrawerOpen } from '../leftDrawer/useDrawerProp'
function drawerOpenBtnClicked() {
  leftDrawerOpen.value = !leftDrawerOpen.value
}
</script>

<style lang="scss" scoped></style>
