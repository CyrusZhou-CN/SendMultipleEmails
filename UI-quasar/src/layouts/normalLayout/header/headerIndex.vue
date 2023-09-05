<template>
  <q-header bordered class="bg-white text-grey-8" height-hint="64">
    <q-toolbar class="GPL__toolbar" style="height: 64px">
      <q-btn flat dense round @click="drawerOpenBtnClicked" aria-label="Menu" icon="menu" class="q-mx-md" />

      <q-toolbar-title v-if="$q.screen.gt.sm" shrink class="row items-center no-wrap">
        <span class="q-ml-sm">SendMultipleEmails</span>
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

const props = defineProps({
  leftDrawerOpen: { type: Boolean, default: () => false }
})
const emit = defineEmits(['update:leftDrawerOpen'])

function drawerOpenBtnClicked() {
  emit('update:leftDrawerOpen', !props.leftDrawerOpen)
}
</script>

<style lang="scss" scoped></style>
