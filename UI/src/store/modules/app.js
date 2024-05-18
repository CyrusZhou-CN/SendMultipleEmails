import { ref } from 'vue';
import Cookies from 'js-cookie';

const sidebarOpened = Cookies.get('sidebarStatus') ? !!+Cookies.get('sidebarStatus') : true;
const sidebarWithoutAnimation = ref(false);
const device = ref('desktop');

const toggleSidebar = () => {
  sidebarOpened.value = !sidebarOpened.value;
  sidebarWithoutAnimation.value = false;
  if (sidebarOpened.value) {
    Cookies.set('sidebarStatus', 1);
  } else {
    Cookies.set('sidebarStatus', 0);
  }
};

const closeSidebar = (withoutAnimation) => {
  Cookies.set('sidebarStatus', 0);
  sidebarOpened.value = false;
  sidebarWithoutAnimation.value = withoutAnimation;
};

const toggleDevice = (newDevice) => {
  device.value = newDevice;
};

export default {
  namespaced: true,
  state() {
    return {
      sidebar: {
        opened: sidebarOpened,
        withoutAnimation: sidebarWithoutAnimation,
      },
      device: device,
    };
  },
  mutations: {
    TOGGLE_SIDEBAR: toggleSidebar,
    CLOSE_SIDEBAR: closeSidebar,
    TOGGLE_DEVICE: toggleDevice,
  },
  actions: {
    toggleSideBar({ commit }) {
      commit('TOGGLE_SIDEBAR');
    },
    closeSideBar({ commit }, { withoutAnimation }) {
      commit('CLOSE_SIDEBAR', withoutAnimation);
    },
    toggleDevice({ commit }, device) {
      commit('TOGGLE_DEVICE', device);
    },
  },
};
