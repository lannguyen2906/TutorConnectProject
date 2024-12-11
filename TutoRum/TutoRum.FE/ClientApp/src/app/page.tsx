import Banner from "./(user)/components/Banner";
import Companies from "./(user)/components/Companies/Companies";
import Courses from "./(user)/components/Courses";
import Mentor from "./(user)/components/Mentor";
import Testimonials from "./(user)/components/Testimonials";
import Newsletter from "./(user)/components/Newsletter/Newsletter";

export default function Home() {
  return (
    <main>
      <Banner />
      <Companies />
      <Courses />
      <Mentor />
      <Testimonials />
      <Newsletter />
    </main>
  );
}
