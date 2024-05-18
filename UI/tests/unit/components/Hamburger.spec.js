import { shallowMount } from '@vue/test-utils';
import Hamburger from '@/components/Hamburger/index.vue';

describe('Hamburger.vue', () => {
  it('toggle click', async () => {
    const wrapper = shallowMount(Hamburger);
    await wrapper.trigger('click');
    expect(wrapper.emitted('toggleClick')).toBeTruthy();
  });

  it('prop isActive', async () => {
    const wrapper = shallowMount(Hamburger, {
      props: {
        isActive: true // 初始化isActive为true
      }
    });

    // 判断是否存在is-active类
    expect(wrapper.classes('is-active')).toBe(true);

    // 更新props
    await wrapper.setProps({ isActive: false });

    // 判断是否存在is-active类
    expect(wrapper.classes('is-active')).toBe(false);
  });
});
