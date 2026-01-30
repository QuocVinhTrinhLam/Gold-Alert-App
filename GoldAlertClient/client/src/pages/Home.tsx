import { PriceDisplay } from "@/components/PriceDisplay";
import { AlertForm } from "@/components/AlertForm";
import { AlertList } from "@/components/AlertList";
import { DollarSign } from "lucide-react";

export default function Home() {
  return (
    <div className="min-h-screen pb-20">
      {/* Header / Hero Section */}
      <header className="bg-white border-b border-green-100 sticky top-0 z-50 bg-opacity-90 backdrop-blur-md">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="w-10 h-10 bg-gradient-to-br from-yellow-400 to-yellow-600 rounded-lg flex items-center justify-center shadow-lg shadow-yellow-500/30">
              <DollarSign className="text-white w-6 h-6" />
            </div>
            <h1 className="text-2xl font-display font-bold text-green-950 tracking-tight">
              Gold<span className="text-yellow-600">Alert</span>
            </h1>
          </div>
          <div className="text-sm font-medium text-green-800 hidden sm:block">
            Ứng dụng Cảnh báo Giá Vàng
          </div>
        </div>
      </header>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
        {/* Real-time Price Section */}
        <section>
          <PriceDisplay />
        </section>

        {/* Main Content Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          {/* Left Column: Form (5 columns) */}
          <div className="lg:col-span-5 space-y-6">
            <AlertForm />
            
            {/* Informational Card */}
            <div className="bg-green-50 rounded-xl p-5 border border-green-100 text-sm text-green-800">
              <h4 className="font-semibold mb-2 flex items-center gap-2">
                <span className="w-2 h-2 rounded-full bg-green-500 animate-pulse"></span>
                Mẹo đầu tư
              </h4>
              <p className="opacity-80 leading-relaxed">
                Đặt cảnh báo giá thấp hơn thị trường để mua vào tích trữ, và giá cao hơn để chốt lời đúng thời điểm.
              </p>
            </div>
          </div>

          {/* Right Column: List (7 columns) */}
          <div className="lg:col-span-7">
            <AlertList />
          </div>
        </div>
      </main>
      
      <footer className="py-6 text-center text-green-900/40 text-sm">
        <p>© 2024 GoldAlert. Mock Data Demo Application.</p>
      </footer>
    </div>
  );
}
