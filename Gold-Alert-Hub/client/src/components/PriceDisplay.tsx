import { useEffect, useState, useRef } from "react";
import { TrendingUp, Clock, AlertTriangle } from "lucide-react";
import { format } from "date-fns";
import { motion, AnimatePresence } from "framer-motion";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useAlerts } from "@/hooks/use-alerts";
import { useToast } from "@/hooks/use-toast";

// Helper for currency formatting
const formatCurrency = (value: number) =>
  new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(value);

export function PriceDisplay() {
  const [price, setPrice] = useState(82500000);
  const [lastUpdated, setLastUpdated] = useState(new Date());
  const [trend, setTrend] = useState<"up" | "down" | "neutral">("neutral");
  
  const { data: alerts } = useAlerts();
  const { toast } = useToast();
  
  // Ref to track notified alerts to avoid spamming toast every 3 seconds for the same alert
  // In a real app, you might want to re-notify after price fluctuates out and back in.
  // For this demo, we'll notify once per "session" or reset after some time.
  const notifiedAlerts = useRef<Set<number>>(new Set());

  useEffect(() => {
    const interval = setInterval(() => {
      setPrice((prev) => {
        const change = (Math.random() - 0.5) * 500000; // Fluctuate +/- 250k
        const newPrice = Math.max(70000000, Math.floor(prev + change));
        
        setTrend(newPrice > prev ? "up" : newPrice < prev ? "down" : "neutral");
        return newPrice;
      });
      setLastUpdated(new Date());
    }, 3000);

    return () => clearInterval(interval);
  }, []);

  // Check alerts whenever price changes
  useEffect(() => {
    if (!alerts) return;

    alerts.forEach((alert) => {
      let triggered = false;
      if (alert.condition === "above" && price >= alert.targetPrice) {
        triggered = true;
      } else if (alert.condition === "below" && price <= alert.targetPrice) {
        triggered = true;
      }

      if (triggered && !notifiedAlerts.current.has(alert.id)) {
        // Play sound
        const audio = new Audio("https://assets.mixkit.co/active_storage/sfx/2869/2869-preview.mp3"); // Simple bell sound
        audio.play().catch(() => {}); // Ignore autoplay errors

        toast({
          title: "CẢNH BÁO GIÁ VÀNG!",
          description: `Giá vàng đã chạm mức ${formatCurrency(price)} (${alert.condition === "above" ? "≥" : "≤"} ${formatCurrency(alert.targetPrice)})`,
          duration: 5000,
          className: "bg-yellow-500 text-black border-none font-bold shadow-xl",
          action: <AlertTriangle className="h-6 w-6 text-black animate-pulse" />,
        });

        notifiedAlerts.current.add(alert.id);
      } else if (!triggered) {
        // Reset notification state if condition is no longer met
        // This allows re-notification if price dips and comes back up
        notifiedAlerts.current.delete(alert.id);
      }
    });
  }, [price, alerts, toast]);

  return (
    <Card className="border-none shadow-xl bg-gradient-to-br from-[#004d25] to-[#00381b] text-white overflow-hidden relative">
      <div className="absolute inset-0 bg-[url('https://www.transparenttextures.com/patterns/cubes.png')] opacity-10"></div>
      
      <CardHeader className="pb-2 relative z-10">
        <div className="flex justify-between items-center">
          <CardTitle className="text-yellow-400 font-display text-xl tracking-wider uppercase flex items-center gap-2">
            <TrendingUp className="w-5 h-5" /> Giá Vàng SJC
          </CardTitle>
          <Badge variant="outline" className="border-yellow-500/30 text-yellow-300 bg-yellow-500/10">
            Trực tuyến
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="relative z-10">
        <div className="flex flex-col md:flex-row items-end md:items-center justify-between gap-4">
          <div className="space-y-1">
            <span className="text-sm text-green-100/70 block mb-1">Giá bán ra (VND/lượng)</span>
            <AnimatePresence mode="wait">
              <motion.div
                key={price}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                className={`text-4xl md:text-5xl font-bold font-mono tracking-tighter tabular-nums ${
                  trend === "up" ? "text-green-400" : trend === "down" ? "text-red-400" : "text-white"
                }`}
              >
                {formatCurrency(price)}
              </motion.div>
            </AnimatePresence>
          </div>
          
          <div className="flex flex-col items-end text-right text-xs text-green-100/50">
            <div className="flex items-center gap-1 mb-1">
              <Clock className="w-3 h-3" />
              Cập nhật: {format(lastUpdated, "HH:mm:ss")}
            </div>
            <div>Hôm nay: {format(lastUpdated, "dd/MM/yyyy")}</div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
