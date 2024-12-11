"use client";
import Courses from "../components/Courses";
import Mentor from "../components/Mentor";
import Testimonials from "../components/Testimonials";
import Newsletter from "../components/Newsletter/Newsletter";
import Banner from "../components/Banner";
import React from "react";
import { FloatButton } from "antd";
import Companies from "../components/Companies/Companies";
import PopularTutors from "../components/PopularTutors";

const page = () => {
  return (
    <div>
      <Banner />
      <Companies />
      <PopularTutors />
      <Newsletter />
      <FloatButton.BackTop />
    </div>
  );
};

export default page;
